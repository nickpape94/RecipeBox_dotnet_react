import React, { useState } from 'react';
import PropTypes from 'prop-types';

const PageNavigation = ({ pagination: { currentPage, itemsPerPage, totalItems, totalPages } }) => {
	console.log(totalPages);
	// const [ numPages, setNumPages ] = useState([]);
	const numPages = [];
	for (let i = 1; i <= totalPages; i++) {
		numPages.push(i);
	}

	console.log(numPages);

	return (
		<div className='pagination'>
			<ul>
				<li>
					<i className='fas fa-angle-double-left' />
				</li>
				<li>
					<i className='fas fa-angle-left' />
				</li>
				{numPages.splice(currentPage - 1, 5).map(
					(value, key) =>
						key === 0 ? (
							<li className='active' key={key}>
								{value}
							</li>
						) : (
							<li key={key}>{value}</li>
						)
				)}
				<li>
					<i className='fas fa-angle-right' />
				</li>
				<li>
					<i className='fas fa-angle-double-right' />
				</li>
			</ul>
		</div>
	);
};

PageNavigation.propTypes = {};

export default PageNavigation;
