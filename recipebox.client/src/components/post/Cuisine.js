import React, { useEffect, useState, Fragment } from 'react';
import { connect } from 'react-redux';
import { getPostsByCuisine, getPosts } from '../../actions/post';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import PageNavigation from '../posts/PageNavigation';

const Cuisine = ({ getPosts, pagination, post: { posts, loading }, match }) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ orderBy, setOrderBy ] = useState('');
	const [ pageNumber, setPageNumber ] = useState(pagination.currentPage !== null ? pagination.currentPage : 1);
	// const [ pageNumber, setPageNumber ] = useState(
	// 	pagination.currentPage === null || (location.state !== undefined && location.state.fromCuisine)
	// 		? 1
	// 		: pagination.currentPage
	// );

	// useEffect(
	// 	() => {
	// 		if (pageNumber !== 1) {
	// 			setPageNumber(1);
	// 		}
	// 	},
	// 	[ orderBy ]
	// );

	useEffect(
		() => {
			const searchParams = match.params.cuisine;
			const userId = '';
			getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
		},
		[ getPosts, orderBy, pageNumber, match.params.cuisine ]
	);

	const onChange = (e) => {
		e.preventDefault();
		setOrderBy(e.target.value);
	};

	if (loading || loadingPage) {
		return <Spinner />;
	}

	return (
		<Fragment>
			<Link to={'/cuisines'} className='btn'>
				<i className='fas fa-arrow-circle-left' /> Back To Cuisines
			</Link>

			<h1>{match.params.cuisine}</h1>
			<h1>{orderBy}</h1>

			<div className='dropdown2'>
				<select
					className='test2'
					name='orderBy'
					type='text'
					value={orderBy}
					required
					onChange={(e) => onChange(e)}
				>
					<option value=''>*Sort by</option>
					<option value='newest'>Newest</option>
					<option value='oldest'>Oldest</option>
					<option value='highest rated'>Highest rated</option>
					<option value='most discussed'>Most discussed</option>
				</select>
			</div>

			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />

			<div className='cards'>
				{posts.map((post) => (
					<PostItem
						key={post.postId}
						post={post}
						postsFromCuisine={true}
						// postPhoto={post.postPhoto.filter((photo) => photo.isMain == true)}
					/>
				))}
			</div>

			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />
		</Fragment>
	);
};

Cuisine.propTypes = {
	getPosts: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	pagination: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	pagination: state.pagination
});

export default connect(mapStateToProps, { getPosts })(Cuisine);
