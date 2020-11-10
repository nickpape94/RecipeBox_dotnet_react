import React, { useEffect, useState, Fragment } from 'react';
import { connect } from 'react-redux';
import { getPostsByCuisine, getPosts } from '../../actions/post';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import PageNavigation from '../posts/PageNavigation';

const Cuisine = ({ getPosts, pagination, post: { posts, loading }, match, history }) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ pageNumber, setPageNumber ] = useState(1);

	useEffect(
		() => {
			const searchParams = match.params.cuisine;
			const orderBy = '';
			const userId = '';
			getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
		},
		[ getPosts, pageNumber, match.params.cuisine ]
	);

	if (loading || loadingPage) {
		return <Spinner />;
	}

	// console.log(history.location.pathname);

	return (
		<Fragment>
			<Link to={'/cuisines'} className='btn'>
				<i className='fas fa-arrow-circle-left' /> Back To Cuisines
			</Link>

			<h1>{match.params.cuisine}</h1>

			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />

			<div className='cards'>
				{posts.map((post) => (
					<PostItem
						key={post.postId}
						post={post}
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
